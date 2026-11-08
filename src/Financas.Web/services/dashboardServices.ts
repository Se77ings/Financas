import { getAsync, readErrorAsync } from "./baseServices";
import DashboardView from "@/models/Dashboard/DashboardView";

/** Sem ano, traz o dashboard de todas as competências. */
export async function getDashboardAsync(period: { year: number; month: number } | null): Promise<DashboardView> {
    const query = period ? `?year=${period.year}&month=${period.month}` : '';
    const response = await getAsync(`api/Dashboard${query}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch dashboard'));
    }

    return await response.json();
}
