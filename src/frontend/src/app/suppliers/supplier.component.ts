import { ChangeDetectionStrategy, Component, DestroyRef, inject } from "@angular/core";
import { SupplierStore } from "./supplier.store";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { SupplierListComponent } from "./ui/supplier-list.component";
import { SupplierModel } from "./supplier.model";
import { SupplierFilterComponent } from "./ui/supplier-filter.comonent";
import { SupplierPaginatorComponent } from "./ui/supplier-paginator.component";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { MatDialog, MatDialogModule } from "@angular/material/dialog";
import { SupplierDialogComponent } from "./ui/supplier-dialog.component";

@Component({
    selector: 'app-supplier',
    imports: [
        MatProgressSpinnerModule,
        MatButtonModule,
        SupplierListComponent,
        SupplierFilterComponent,
        SupplierPaginatorComponent,
        MatDialogModule
    ],
    providers: [SupplierStore],
    template: `
       <div style="display: flex;align-items:center;gap:5px;margin-bottom:8px">
        <span style="font-size: 26px;font-weight:bold"> Suppliers </span>
        <button style="margin:10px 0px"
        mat-raised-button
        color="primary"
        (click)="onAddUpdate('Add Product',null)"
      >
        Add More
      </button>
      </div>

      @if(store.loading()){
        <div class="spinner-center">
          <mat-spinner diameter="50"></mat-spinner>
        </div>
      }
      
    <app-supplier-filter (filter)="onFilter($event)"/>

     @if(store.suppliers() && store.suppliers().length > 0){

         <app-supplier-list [suppliers]="store.suppliers()" (sort)="onSort($event)" (edit)="onAddUpdate('Edit purchase', $event)" (delete)="onDelete($event)"/>

         <app-supplier-paginator [totalRecords]="store.totalRecords()" (pageSelect)="onPageSelect($event)"/>
     }
     @else {
         <p style="margin-top:20px;font-size:21px">
          No records found
        </p>
     }
    `,
    styles: [``],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierComponent {
    store = inject(SupplierStore);
    destroyRef = inject(DestroyRef);
    dialog = inject(MatDialog);

    onAddUpdate(
        action: string,
        supplier: SupplierModel | null = null
    ) {
        const dialogRef = this.dialog.open(SupplierDialogComponent, {
            data: { supplier, title: action + " Supplier" },
        });

        dialogRef.componentInstance.submit
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((submittedData) => {
                if (!submittedData) return;
                if (submittedData.id && submittedData.id > 0) {
                    this.store.updateSupplier(submittedData);
                } else {
                    this.store.addSupplier(submittedData);
                }
                dialogRef.componentInstance.supplierForm.reset();
                dialogRef.componentInstance.onCanceled();
            });
    }

    onPageSelect = (pageData: { page: number; limit: number }) => {
        this.store.setPagination(pageData);
    }

    onFilter = (searchTerm: string) => this.store.setSearch(searchTerm);

    onSort = (sortingData: { sortColumn: string; sortDirection: "asc" | "desc" }) => this.store.setSorting(sortingData);

    // onEdit = (supplier: SupplierModel) => {
    //     const suppliers = this.store.suppliers() as readonly SupplierModel[];
    //     this.onAddUpdate('Edit purchase', supplier, suppliers)
    // }

    onDelete = (supplier: SupplierModel) => {
        if (!confirm('Are you sure to delete supplier ' + supplier.supplierName))
            return;
        this.store.deleteSupplier(supplier.id);
    }
}