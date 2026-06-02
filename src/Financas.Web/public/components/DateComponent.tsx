import { toInputDate } from '@/utils/dateFormat';

export function DateComponent({
    date,
    onChange,
    tabIndex,
    plustabindex,
    minustabindex
}: {
    date: string,
    onChange: (value: string) => void,
    tabIndex?: number,
    plustabindex?: number,
    minustabindex?: number
}) {
    const valueToShow = toInputDate(date);
    const activeDate = valueToShow || new Date().toISOString().split('T')[0];

    const incrementDate = () => {
        const [year, month, day] = activeDate.split('-').map(Number);
        const dateObj = new Date(year, month - 1, day);
        dateObj.setDate(dateObj.getDate() + 1);
        const y = dateObj.getFullYear();
        const m = String(dateObj.getMonth() + 1).padStart(2, '0');
        const d = String(dateObj.getDate()).padStart(2, '0');
        onChange(`${y}-${m}-${d}`);
    };

    const decrementDate = () => {
        const [year, month, day] = activeDate.split('-').map(Number);
        const dateObj = new Date(year, month - 1, day);
        dateObj.setDate(dateObj.getDate() - 1);
        const y = dateObj.getFullYear();
        const m = String(dateObj.getMonth() + 1).padStart(2, '0');
        const d = String(dateObj.getDate()).padStart(2, '0');
        onChange(`${y}-${m}-${d}`);
    };

    return (
        <div className='flex flex-row gap-1'>
            <button tabIndex={minustabindex} className="flex justify-center items-center flex-row px-4 bg-[#211f1c] border border-white/5 rounded cursor-pointer" style={{ width: 20 }} type='button' onClick={decrementDate}>-</button>
            <input
                tabIndex={tabIndex}
                type="date"
                name="date"
                value={valueToShow}
                onChange={(e) => onChange(e.target.value)}
                required
                className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
            />
            <button tabIndex={plustabindex} className="flex justify-center items-center flex-row px-4 bg-[#211f1c] border border-white/5 rounded cursor-pointer" style={{ width: 20 }} type='button' onClick={incrementDate}>+</button>
        </div>
    );
}