import { ChangeDetectionStrategy, Component, EventEmitter, Output } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { catchError, debounceTime, of, tap } from "rxjs";

@Component({
    selector: 'app-supplier-filter',
    imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule],
    template: `
      <mat-form-field [appearance]="'outline'" style="width: 400px;">
          <mat-label>Search supplier or contact person</mat-label>
          <input matInput [formControl]="searchFilter"/>
      </mat-form-field>
    `,
    styles: [``],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SupplierFilterComponent {
    searchFilter = new FormControl<string>('');
    @Output() filter = new EventEmitter<string>();
    constructor() {
        this.searchFilter.valueChanges.pipe(
            debounceTime(300),
            tap(val => {
                this.filter.emit(val ?? '');
            }),
            catchError((err) => {
                console.log(err);
                return of(err);
            }),
            takeUntilDestroyed()
        ).subscribe();
    }
}