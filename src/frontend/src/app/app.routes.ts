import { Routes } from "@angular/router";
import { LoginComponent } from "./auth/login/login.component";

export const routes: Routes = [
  {
    path: "login",
    component: LoginComponent
  },
  {
    path: "dashboard",
    loadComponent: () => import("./dashboard.component").then(a => a.DashbardComponent),
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
    redirectTo: "/login",
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
