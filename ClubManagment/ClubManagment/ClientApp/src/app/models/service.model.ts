import { Permission } from './permission.model';


export class Service {

    constructor(name?: string, description?: string, price?: number) {

        this.name = name;
        this.description = description;
        this.price = price;
    }

    public id: string;
    public name: string;
    public description: string;
    public usersCount: number;
    public price    : number;
}
