import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_Models/Pagination";
import { UserParams } from "../_Models/userParam";

 export function getPaginatedResults<T>(url: string, params: HttpParams, http: HttpClient) {

    const paginatedresult: PaginatedResult<T> = new PaginatedResult<T> ();

    return http.get<T>(url, { observe: 'response', params }).pipe(
      map((response:any) => {

        if (response.headers.get("Pagination") !== null) {
          //@ts-ignore
          paginatedresult.result = response.body;
          //@ts-ignore
          paginatedresult.pagination = JSON.parse(response.headers.get("Pagination"));

          console.log(paginatedresult);
        }
        return paginatedresult;
      })
    );
  }

  export function getPaginationHeader(userparam: UserParams) : HttpParams{
    let params = new HttpParams();
    
    params = params.append("pageNumber", userparam.pageNumber.toString());
    params = params.append("pageSize", userparam.pageSize.toString());

    if(userparam.pagingType === "LIKE")
    {
      params = params.append("predicate", userparam.predicate);
      return params;
    }
    else if(userparam.pagingType === "MESSAGE")
    {
      params = params.append("container", userparam.container);
    }
    else if(userparam.pagingType ===  "USER")
    {
        params = params.append("minAge", userparam.minAge.toString());
        params = params.append("maxAge", userparam.maxAge.toString());
        params = params.append("gender", userparam.gender);
        params = params.append("orderBy", userparam.orderBy);
    }

    return params;
  }