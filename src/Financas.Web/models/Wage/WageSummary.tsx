export default class WageSummaryView {
	id: string;
	grossValue: number;
	netValue: number;
	paymentDate: string;

	constructor(id: string, grossValue: number, netValue: number, paymentDate: string) {
		this.id = id;
		this.grossValue = grossValue;
		this.netValue = netValue;
		this.paymentDate = paymentDate;
	}

	static fromJSON(data: any): WageSummaryView {
		return new WageSummaryView(data.id, data.grossValue, data.netValue, data.paymentDate);
	}
}
