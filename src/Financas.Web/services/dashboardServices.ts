import { getAsync, readErrorAsync } from "./baseServices";
import DashboardView from "@/models/Dashboard/DashboardView";

export async function getDashboardAsync(effectiveDateId: string): Promise<DashboardView> {
    const response = await getAsync(`api/Dashboard/${effectiveDateId}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch dashboard'));
    }

    return await response.json();
}
