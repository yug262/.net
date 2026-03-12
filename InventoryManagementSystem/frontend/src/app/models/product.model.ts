export interface Product {
  id: number;
  productName: string;
  sku: string;
  categoryId: number;
  categoryName?: string;
  purchasePrice: number;
  sellingPrice: number;
  quantity: number;
  description?: string;
  createdAt: string;
}
