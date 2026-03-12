import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../../services/category.service';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.css'
})
export class CategoryFormComponent implements OnInit {
  categoryName = '';
  isEditMode = false;
  categoryId: number | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.categoryId = +id;
      this.categoryService.getById(this.categoryId).subscribe({
        next: (cat) => {
          this.categoryName = cat.categoryName;
        },
        error: () => {
          this.errorMessage = 'Category not found';
        }
      });
    }
  }

  onSubmit(): void {
    this.isLoading = true;
    const payload = { categoryName: this.categoryName };

    if (this.isEditMode && this.categoryId) {
      this.categoryService.update(this.categoryId, payload).subscribe({
        next: () => this.router.navigate(['/categories']),
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
      this.categoryService.create(payload).subscribe({
        next: () => this.router.navigate(['/categories']),
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
