import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../services/order.service';
import { ProductService } from '../../services/product.service';
import { Order, OrderCreate } from '../../models/order.model';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  products: Product[] = [];

  // Form fields
  selectedProductId: number | null = null;
  orderQuantity: number = 1;

  isLoadingOrders = true;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private orderService: OrderService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadOrders();
    this.loadAvailableProducts();
  }

  loadOrders(): void {
    this.orderService.getAll().subscribe({
      next: (data: Order[]) => {
        this.orders = data;
        this.isLoadingOrders = false;
      },
      error: () => { this.isLoadingOrders = false; }
    });
  }

  loadAvailableProducts(): void {
    this.productService.getAll().subscribe({
      next: (data: Product[]) => {
        // Only show products with stock > 0
        this.products = data.filter((p: Product) => p.quantity > 0);
      }
    });
  }

  get selectedProduct(): Product | undefined {
    return this.products.find(p => p.id === Number(this.selectedProductId));
  }

  get maxQuantity(): number {
    return this.selectedProduct?.quantity ?? 1;
  }

  get estimatedRevenue(): number {
    return (this.selectedProduct?.sellingPrice ?? 0) * (this.orderQuantity || 0);
  }

  placeOrder(): void {
    if (!this.selectedProductId || this.orderQuantity < 1) return;

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const dto: OrderCreate = {
      productId: Number(this.selectedProductId),
      quantity: this.orderQuantity
    };

    this.orderService.create(dto).subscribe({
      next: (order: Order) => {
        this.successMessage = `Order placed! ${order.quantity}x ${order.productName} sold for ₹${order.totalRevenue.toFixed(2)}`;
        this.isSubmitting = false;
        this.selectedProductId = null;
        this.orderQuantity = 1;
        this.loadOrders();
        this.loadAvailableProducts(); // Refresh stock counts
        setTimeout(() => this.successMessage = '', 4000);
      },
      error: (err: { error?: { message?: string } }) => {
        this.errorMessage = err.error?.message || 'Failed to place order.';
        this.isSubmitting = false;
      }
    });
  }

  cancelOrder(id: number): void {
    if (!confirm('Cancel this order? Stock will be restored.')) return;

    this.orderService.delete(id).subscribe({
      next: () => {
        this.orders = this.orders.filter(o => o.id !== id);
        this.loadAvailableProducts(); // Refresh stock
      },
      error: (err: { error?: { message?: string } }) => {
        this.errorMessage = err.error?.message || 'Failed to cancel order.';
      }
    });
  }

  get totalRevenue(): number {
    return this.orders.reduce((sum, o) => sum + o.totalRevenue, 0);
  }

  get totalProfit(): number {
    return this.orders.reduce((sum, o) => sum + o.totalProfit, 0);
  }
}
