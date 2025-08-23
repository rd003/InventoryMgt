import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  inject,
} from "@angular/core";
import { provideComponentStore } from "@ngrx/component-store";
import { CategoryStore } from "../category/category.store";
import { ProductStore } from "./product.store";
import { AsyncPipe } from "@angular/common";
import { ProductListComponent } from "./ui/product-list.component";
import { ProductFilterComponent } from "./ui/product-filter.component";
import { Product } from "./product.model";
import { ProductPaginatorComponent } from "./ui/product-paginator.component";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatDialog } from "@angular/material/dialog";
import { Subject, takeUntil, tap } from "rxjs";
import { ProductDialogComponent } from "./ui/product-dialog.component";
import { CategoryModel } from "../category/category.model";
import { MatButtonModule } from "@angular/material/button";
import { capitalize } from "../utils/init-cap.util";
import { SupplierModel } from "../suppliers/supplier.model";
import { SupplierStore } from "../suppliers/supplier.store";

@Component({
  selector: "app-product",
  imports: [
    AsyncPipe,
    ProductListComponent,
    ProductFilterComponent,
    ProductPaginatorComponent,
    MatProgressSpinnerModule,
    MatButtonModule,
  ],
  providers: [
    provideComponentStore(CategoryStore),
    provideComponentStore(ProductStore),
    SupplierStore
  ],
  template: `
      <div style="display: flex;align-items:center;gap:5px;margin-bottom:8px">
        <span style="font-size: 26px;font-weight:bold"> Products </span>
        <button style="margin:10px 0px"
        mat-raised-button
        color="primary"
        (click)="onAddUpdate('Add Product')"
      >
        Add More
      </button>
      </div>

    @if(vm$ | async; as vm)
    {
      @if(vm.loading){
        <div class="spinner-center">
          <mat-spinner diameter="50"></mat-spinner>
        </div>
      }
        <app-product-filter (filter)="onSearch($event)" />
      
      @if(vm.products && vm.products.length > 0){

        <app-product-list
          [products]="vm.products"
          (edit)="onAddUpdate('Update Product', $event)"
          (delete)="onDelete($event)"
          (sort)="onSort($event)"
        />
        <app-product-paginator
          (pageSelect)="onPageSelect($event)"
          [totalRecords]="vm.totalRecords"
        />
      }
      @else{
        <p style="margin-top:20px;font-size:21px">
          No records found
        </p>
      }
    }
  `,
  styles: [``],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductComponent implements OnDestroy {
  productStore = inject(ProductStore);
  categoryStore = inject(CategoryStore);
  supplierStore = inject(SupplierStore);
  dialog = inject(MatDialog);
  destroyed$ = new Subject<boolean>();

  vm$ = this.productStore.vm$;

  onPageSelect(pageData: { page: number; limit: number }) {
    this.productStore.setPage(pageData.page);
    this.productStore.setPageLimit(pageData.limit);
  }

  onSearch(search: string | null) {
    this.productStore.setSearchTerm(search);
  }

  onDelete(product: Product) {
    if (confirm("Are you sure to delete?")) {
      this.productStore.deleteProduct(product.id);
    }
    // console.log(product);
  }

  onAddUpdate(action: string, product: Product | null = null) {
    let categories: CategoryModel[] = [];
    let suppliers: readonly SupplierModel[] = this.supplierStore.suppliers();

    this.categoryStore.vm$
      .pipe(
        takeUntil(this.destroyed$),
        tap((a) => {
          categories = a.categories;
        })
      )
      .subscribe();


    const dialogRef = this.dialog.open(ProductDialogComponent, {
      data: { product, title: action, categories, suppliers },
    });

    dialogRef.componentInstance.sumbit
      .pipe(takeUntil(this.destroyed$))
      .subscribe((submittedProduct) => {
        if (!submittedProduct) return;
        if (submittedProduct.id && submittedProduct.id > 0) {
          // update book
          //console.log("update");
          this.productStore.updateProduct(submittedProduct);
        } else {
          // add book
          //console.log(submittedProduct);
          this.productStore.addProduct(submittedProduct);
        }
        dialogRef.componentInstance.productForm.reset();
        dialogRef.componentInstance.onCanceled();
      });
  }

  onSort(sortObj: { sortColumn: string; sortDirection: "asc" | "desc" }) {
    this.productStore.setSortDirection(sortObj.sortDirection);
    this.productStore.setSortColumn(capitalize(sortObj.sortColumn));
  }

  constructor() { }
  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.unsubscribe();
  }
}
