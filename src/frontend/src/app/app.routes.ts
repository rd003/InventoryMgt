import { Routes } from "@angular/router";
import { DashbardComponent } from "./dashboard.component";

export const routes: Routes = [
  {
    path: "dashboard",
    component: DashbardComponent,
  },
  {
    path: "categories",
    async loadComponent() {
      const a = await import("./category/category.component");
      return a.CategoryComponent;
    },
  },
  {
    path: "products",
    async loadComponent() {
      const a = await import("./products/product.component");
      return a.ProductComponent;
    },
  },
  {
    path: "purchases",
    async loadComponent() {
      const a = await import("./purchase/purchase.component");
      return a.PurchaseComponent;
    },
  },
  {
    path: "suppliers",
    loadComponent: () => import("./suppliers/supplier.component").then(s => s.SupplierComponent)
  },
  {
    path: "",
    redirectTo: "/dashboard",
    pathMatch: "full",
  },
  {
    path: "sales",
    loadComponent: () =>
      import("./sales/sale.component").then((c) => c.SaleComponent),
  },
  {
    path: "stock",
    loadComponent: () =>
      import("./stocks/stock.component").then((a) => a.StockComponent),
  },
  {
    path: "**",
    async loadComponent() {
      const a = await import("./not-found.component");
      return a.NotFoundComponent;
    },
  },
];
