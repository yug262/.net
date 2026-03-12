import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../models/category.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.css'
})
export class ProductFormComponent implements OnInit {
  productForm!: FormGroup;
  categories: Category[] = [];
  isEditMode = false;
  productId: number | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.productForm = this.fb.group({
      productName: ['', [Validators.required, Validators.maxLength(200)]],
      sku: ['', [Validators.required, Validators.maxLength(50)]],
      categoryId: ['', [Validators.required]],
      purchasePrice: [0, [Validators.required, Validators.min(0)]],
      sellingPrice: [0, [Validators.required, Validators.min(0.01)]],
      quantity: [0, [Validators.required, Validators.min(0)]],
      description: ['']
    });

    // Load categories for dropdown
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories = data
    });

    // Check if edit mode
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.productId = +id;
      this.productService.getById(this.productId).subscribe({
        next: (product) => {
          this.productForm.patchValue({
            productName: product.productName,
            sku: product.sku,
            categoryId: product.categoryId,
            purchasePrice: product.purchasePrice,
            sellingPrice: product.sellingPrice,
            quantity: product.quantity,
            description: product.description
          });
        },
        error: () => {
          this.errorMessage = 'Product not found';
        }
      });
    }
  }

  onSubmit(): void {
    if (this.productForm.invalid) return;

    this.isLoading = true;
    const formData = this.productForm.value;

    if (this.isEditMode && this.productId) {
      this.productService.update(this.productId, formData).subscribe({
        next: () => this.router.navigate(['/products']),
        error: (err) => {
          this.isLoading = false;
          let msg = 'Update failed';
          if (err.error?.errors) msg = Object.values(err.error.errors).flat().join(', ');
          else if (err.error?.message) msg = err.error.message;
          else if (err.error?.title) msg = err.error.title;
          this.errorMessage = msg;
        }
      });
    } else {
      this.productService.create(formData).subscribe({
        next: () => this.router.navigate(['/products']),
        error: (err) => {
          this.isLoading = false;
          let msg = 'Create failed';
          if (err.error?.errors) msg = Object.values(err.error.errors).flat().join(', ');
          else if (err.error?.message) msg = err.error.message;
          else if (err.error?.title) msg = err.error.title;
          this.errorMessage = msg;
        }
      });
    }
  }
}
