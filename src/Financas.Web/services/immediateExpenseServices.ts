import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";
import ImmediateExpenseSummary from "@/models/ImmediateExpense/ImmediateExpenseSummary";
import { Deserializer } from "./Deserializer";

export interface ImmediateExpenseRequest {
    description: string;
    dueDate: string;
    subTotal: number;
    discounts: number;
    observation: string;
    effectiveDateId: string;
    categoryId: string | null;
    subscriptionFrequencyType?: number;
    subscriptionStartDate?: string;
    subscriptionEndDate?: string | null;
    isSubscription?: boolean;
}

export async function getImmediateExpensesAsync(): Promise<ImmediateExpenseSummary[]> {
    const response = await getAsync(`api/ImmediateExpense`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch immediate expenses: ${response.statusText}`);
    }

    const data = await response.json();
    
    return Deserializer.deserializeArray(data, ImmediateExpenseSummary);
}

export async function getImmediateExpenseByIdAsync(id: string): Promise<ImmediateExpenseSummary> {
    const response = await getAsync(`api/ImmediateExpense/${id}`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch immediate expense: ${response.statusText}`);
    }

    const data = await response.json();
    
    return Deserializer.deserialize(data, ImmediateExpenseSummary);
}

export async function addImmediateExpenseAsync(request: ImmediateExpenseRequest): Promise<void> {
    const response = await postAsync(`api/ImmediateExpense`, request);
    
    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to add immediate expense'));
    }
}

export async function updateImmediateExpenseAsync(id: string, request: ImmediateExpenseRequest): Promise<void> {
    const response = await putAsync(`api/ImmediateExpense/${id}`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to update immediate expense'));
    }
}

export async function deleteImmediateExpenseAsync(id: string): Promise<void> {
    const response = await deleteAsync(`api/ImmediateExpense/${id}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to delete immediate expense'));
    }
}
