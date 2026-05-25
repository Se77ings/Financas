export default class CardExpenseItemSummary {
    id: string;
    description: string;
    total: number;
    dueDate: string;
    discounts: number;
    observation: string;
    currentInstallments: number;
    quantityInstallments: number;
    isChecked: boolean;
    displayDescription: string;

    constructor(id: string, description: string, total: number, dueDate: string, discounts: number, observation: string, currentInstallments: number, quantityInstallments: number, isChecked: boolean, displayDescription: string) {
        this.id = id;
        this.description = description;
        this.total = total;
        this.dueDate = dueDate;
        this.discounts = discounts;
        this.observation = observation;
        this.currentInstallments = currentInstallments;
        this.quantityInstallments = quantityInstallments;
        this.isChecked = isChecked;
        this.displayDescription = displayDescription;

    }

    static fromJSON(data: any): CardExpenseItemSummary {
        return new CardExpenseItemSummary(data.id, data.description, data.total, data.dueDate, data.discounts, data.observation, data.currentInstallments, data.quantityInstallments, data.isChecked, data.displayDescription);
    }
}