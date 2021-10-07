import { User } from "./user";

export class UserParams{
    gender: string;
    minAge: number = 18;
    maxAge: number = 99;
    pageNumber: number = 1;
    pageSize = 3;

    orderBy = 'lastActive';

    predicate = "liked"

    constructor(user: User){
        this.gender = user.gender === 'male' ? 'female' : 'male';
    }

    pagingType: string =  "USER";

    container = "Unread";
}