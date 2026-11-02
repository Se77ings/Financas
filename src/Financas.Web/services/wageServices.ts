import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";
import WageSummaryView from "@/models/Wage/WageSummary";
import WageView, { DeductionModel } from "@/models/Wage/WageView";
import { Deserializer } from "./Deserializer";

export interface WageRequest {
	grossValue: number;
	netValue: number;
	paymentDate: string;
	deductions: DeductionModel[];
}

export async function getWagesAsync(): Promise<WageSummaryView[]> {
	const response = await getAsync(`api/Wage`);
	
	if (!response.ok) {
		const errData = await response.json().catch(() => null);
		const details = errData?.details || errData?.message || response.statusText;
		throw new Error(`Failed to fetch wages: ${details}`);
	}

	const data = await response.json();
	return Deserializer.deserializeArray(data, WageSummaryView);
}

export async function getWageByIdAsync(id: string): Promise<WageView> {
	const response = await getAsync(`api/Wage/${id}`);
	
	if (!response.ok) {
		const errData = await response.json().catch(() => null);
		const details = errData?.details || errData?.message || response.statusText;
		throw new Error(`Failed to fetch wage: ${details}`);
	}

	const data = await response.json();
	return Deserializer.deserialize(data, WageView);
}

export async function addWageAsync(request: WageRequest): Promise<void> {
	const response = await postAsync(`api/Wage`, request);
	
	if (!response.ok) {
		throw new Error(await readErrorAsync(response, 'Failed to add wage'));
	}
}

export async function updateWageAsync(id: string, request: WageRequest): Promise<void> {
	const response = await putAsync(`api/Wage/${id}`, request);

	if (!response.ok) {
		throw new Error(await readErrorAsync(response, 'Failed to update wage'));
	}
}

export async function deleteWageAsync(id: string): Promise<void> {
	const response = await deleteAsync(`api/Wage/${id}`);

	if (!response.ok) {
		throw new Error(await readErrorAsync(response, 'Failed to delete wage'));
	}
}
