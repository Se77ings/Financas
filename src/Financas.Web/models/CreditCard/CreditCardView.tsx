export default class CreditCardView {
    id: string;
    name: string;
    closingDay: number;
    dueDay: number;
    limit: number;
    isActive: boolean;

    constructor(id: string, name: string, closingDay: number, dueDay: number, limit: number, isActive: boolean) {
        this.id = id;
        this.name = name;
        this.closingDay = closingDay;
        this.dueDay = dueDay;
        this.limit = limit;
        this.isActive = isActive;
    }

    static fromJSON(data: any): CreditCardView {
        return new CreditCardView(
            data.id,
            data.name,
            data.closingDay,
            data.dueDay,
            data.limit,
            data.isActive
        );
    }
}
