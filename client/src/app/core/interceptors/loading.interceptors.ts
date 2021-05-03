import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { delay, finalize } from "rxjs/operators";
import { BusyService } from "../services/busy.service";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor{

    constructor(private busySevice: BusyService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>>
    {
        this.busySevice.busy();
        return next.handle(req).pipe(
            delay(0),
            finalize(() => {
                this.busySevice.idle();
            })
        );
    }

}