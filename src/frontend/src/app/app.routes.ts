import { Routes } from "@angular/router";
import { LoginComponent } from "./auth/login/login.component";
import { authGuard } from "./guards/auth.guard";

export const routes: Routes = [
  {
    path: "login",
    component: LoginComponent
  },
  {
    path: "dashboard",
    loadComponent: () => import("./dashboard.component").then(a => a.DashbardComponent),
    canActivate: [authGuard]
  },
  {
    path: "categories",
    async loadComponent() {
      const a = await import("./category/category.component");
      return a.CategoryComponent;
    },
    canActivate: [authGuard]
  },
  {
    path: "products",
    async loadComponent() {
      const a = await import("./products/product.component");
      return a.ProductComponent;
    },
    canActivate: [authGuard]
  },
  {
    path: "purchases",
    async loadComponent() {
      const a = await import("./purchase/purchase.component");
      return a.PurchaseComponent;
    },
    canActivate: [authGuard]
  },
  {
    path: "suppliers",
    loadComponent: () => import("./suppliers/supplier.component").then(s => s.SupplierComponent),
    canActivate: [authGuard]
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
    canActivate: [authGuard]
  },
  {
    path: "stock",
    loadComponent: () =>
      import("./stocks/stock.component").then((a) => a.StockComponent),
    canActivate: [authGuard]
  },
  {
    path: "**",
    async loadComponent() {
      const a = await import("./not-found.component");
      return a.NotFoundComponent;
    },
  },
];
