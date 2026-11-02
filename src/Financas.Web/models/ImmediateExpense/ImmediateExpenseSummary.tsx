export default class ImmediateExpenseSummary {
    id: string
    description: string;
    total: number;
    dueDate: string;
    subTotal: number;
    discounts: number;
    observation: string;
    effectiveDateId: string;
    categoryId: string | null;

    constructor(id: string, description: string, total: number, dueDate: string, subTotal: number, discounts: number, observation: string, effectiveDateId: string, categoryId: string | null) {
        this.id = id;
        this.description = description;
        this.total = total;
        this.dueDate = dueDate;
        this.subTotal = subTotal;
        this.discounts = discounts;
        this.observation = observation;
        this.effectiveDateId = effectiveDateId;
        this.categoryId = categoryId;
    }

    static fromJSON(data: any): ImmediateExpenseSummary {
        return new ImmediateExpenseSummary(data.id, data.description, data.total, data.dueDate, data.subTotal, data.discounts, data.observation, data.effectiveDateId, data.categoryId ?? null);
    }
}

