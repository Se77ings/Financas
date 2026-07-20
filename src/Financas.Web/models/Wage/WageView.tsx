export interface DeductionModel {
	description: string;
	value: number;
}

export default class WageView {
	id: string;
	grossValue: number;
	netValue: number;
	paymentDate: string;
	deductions: DeductionModel[];

	constructor(id: string, grossValue: number, netValue: number, paymentDate: string, deductions: DeductionModel[]) {
		this.id = id;
		this.grossValue = grossValue;
		this.netValue = netValue;
		this.paymentDate = paymentDate;
		this.deductions = deductions;
	}

	static fromJSON(data: any): WageView {
		return new WageView(
			data.id,
			data.grossValue,
			data.netValue,
			data.paymentDate,
			data.deductions || []
		);
	}
}
