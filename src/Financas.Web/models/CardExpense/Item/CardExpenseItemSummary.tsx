export default class CardExpenseItemSummary {
    id: string;
    description: string;
    total: number;
    dueDate: string;
    discounts: number;
    observation: string;
    currentInstallment: number;
    quantityInstallments: number;
    isChecked: boolean;
    displayDescription: string;
    categoryId: string | null;

    constructor(id: string, description: string, total: number, dueDate: string, discounts: number, observation: string, currentInstallment: number, quantityInstallments: number, isChecked: boolean, displayDescription: string, categoryId: string | null) {
        this.id = id;
        this.description = description;
        this.total = total;
        this.dueDate = dueDate;
        this.discounts = discounts;
        this.observation = observation;
        this.currentInstallment = currentInstallment;
        this.quantityInstallments = quantityInstallments;
        this.isChecked = isChecked;
        this.displayDescription = displayDescription;
        this.categoryId = categoryId;

    }

    static fromJSON(data: any): CardExpenseItemSummary {
        return new CardExpenseItemSummary(data.id, data.description, data.total, data.dueDate, data.discounts, data.observation, data.currentInstallment, data.quantityInstallments, data.isChecked, data.displayDescription, data.categoryId ?? null);
    }
}