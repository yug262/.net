export interface Order {
  id: number;
  productId: number;
  productName: string;
  sku: string;
  quantity: number;
  unitSellingPrice: number;
  unitPurchasePrice: number;
  totalRevenue: number;
  totalProfit: number;
  createdAt: string;
}

export interface OrderCreate {
  productId: number;
  quantity: number;
}
