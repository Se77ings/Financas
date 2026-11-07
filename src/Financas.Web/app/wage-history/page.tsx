"use client"

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import {
	getWagesAsync,
	getWageByIdAsync,
	addWageAsync,
	updateWageAsync,
	deleteWageAsync,
	WageRequest
} from "@/services/wageServices";
import WageSummaryView from "@/models/Wage/WageSummary";
import WageView, { DeductionModel } from "@/models/Wage/WageView";
import { IS_DESKTOP } from "@/utils/appMode";
import { todayInputDate } from "@/utils/dateFormat";

export default function WageHistoryPage() {
	const router = useRouter();
	const [wages, setWages] = useState<WageSummaryView[]>([]);
	const [isLoading, setIsLoading] = useState(true);
	const [errorMsg, setErrorMsg] = useState("");

	// Modal State
	const [isModalOpen, setIsModalOpen] = useState(false);
	const [editingWageId, setEditingWageId] = useState<string | null>(null);
	const [grossValue, setGrossValue] = useState("");
	const [paymentDate, setPaymentDate] = useState("");
	const [deductions, setDeductions] = useState<DeductionModel[]>([]);

	// Deduction form state inside modal
	const [deductionDesc, setDeductionDesc] = useState("");
	const [deductionValue, setDeductionValue] = useState("");

	// Load Wages
	const loadWages = async () => {
		setIsLoading(true);
		setErrorMsg("");
		try {
			const data = await getWagesAsync();
			// Sort by payment date descending
			data.sort((a, b) => new Date(b.paymentDate).getTime() - new Date(a.paymentDate).getTime());
			setWages(data);
		} catch (err: any) {
			console.error(err);
			setErrorMsg(err.message || "Erro ao carregar o histórico de salários.");
		} finally {
			setIsLoading(false);
		}
	};

	useEffect(() => {
		// O programa fechado não tem esta tela: ela só existe lá porque o export gera todas as rotas.
		if (IS_DESKTOP) {
			router.replace("/");
			return;
		}
		loadWages();
	}, [router]);

	// Format currency
	const formatCurrency = (val: number) => {
		return new Intl.NumberFormat("pt-BR", {
			style: "currency",
			currency: "BRL"
		}).format(val);
	};

	// Format date (yyyy-mm-dd to dd/mm/yyyy)
	const formatDate = (dateStr: string) => {
		if (!dateStr) return "";
		const [year, month, day] = dateStr.split("-");
		return `${day}/${month}/${year}`;
	};

	// Calculate net value on the fly
	const calculateNetValue = () => {
		const gross = parseFloat(grossValue) || 0;
		const totalDeductions = deductions.reduce((sum, d) => sum + d.value, 0);
		return Math.max(0, gross - totalDeductions);
	};

	// Open modal for Create
	const handleOpenCreate = () => {
		setEditingWageId(null);
		setGrossValue("");
		setPaymentDate(todayInputDate());
		setDeductions([]);
		setDeductionDesc("");
		setDeductionValue("");
		setIsModalOpen(true);
	};

	// Open modal for Edit
	const handleOpenEdit = async (id: string) => {
		try {
			const wageDetails = await getWageByIdAsync(id);
			setEditingWageId(id);
			setGrossValue(wageDetails.grossValue.toString());
			setPaymentDate(wageDetails.paymentDate);
			setDeductions(wageDetails.deductions.map(d => ({ description: d.description, value: d.value })));
			setDeductionDesc("");
			setDeductionValue("");
			setIsModalOpen(true);
		} catch (err: any) {
			alert(err.message || "Erro ao carregar detalhes do salário.");
		}
	};

	// Add deduction to temporary list in modal
	const handleAddDeduction = () => {
		if (!deductionDesc || !deductionValue) return;
		const val = parseFloat(deductionValue);
		if (isNaN(val) || val <= 0) return;

		setDeductions([...deductions, { description: deductionDesc, value: val }]);
		setDeductionDesc("");
		setDeductionValue("");
	};

	// Remove deduction from temporary list
	const handleRemoveDeduction = (index: number) => {
		setDeductions(deductions.filter((_, i) => i !== index));
	};

	// Save Wage
	const handleSave = async (e: React.FormEvent) => {
		e.preventDefault();
		const gross = parseFloat(grossValue);
		if (isNaN(gross) || gross <= 0 || !paymentDate) {
			alert("Preencha todos os campos obrigatórios com valores válidos.");
			return;
		}

		const request: WageRequest = {
			grossValue: gross,
			netValue: calculateNetValue(),
			paymentDate,
			deductions
		};

		try {
			if (editingWageId) {
				await updateWageAsync(editingWageId, request);
			} else {
				await addWageAsync(request);
			}
			setIsModalOpen(false);
			loadWages();
		} catch (err: any) {
			alert(err.message || "Erro ao salvar salário.");
		}
	};

	// Delete Wage
	const handleDelete = async (id: string) => {
		if (!confirm("Tem certeza que deseja excluir este registro de salário?")) return;
		try {
			await deleteWageAsync(id);
			loadWages();
		} catch (err: any) {
			alert(err.message || "Erro ao excluir registro de salário.");
		}
	};

	return (
		<div className="flex flex-col flex-1 min-h-0 w-full h-full text-gray-100 bg-blue-950">
			{/* Header */}
			<header className="px-5 py-4 border-b border-white/10 shrink-0 flex items-center justify-between bg-blue-900/30">
				<div className="flex items-center gap-3">
					<button
						onClick={() => router.push("/")}
						className="p-1.5 rounded-lg text-gray-400 hover:text-white hover:bg-white/10 transition cursor-pointer"
						title="Voltar ao início"
					>
						<svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
							<path strokeLinecap="round" strokeLinejoin="round" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
						</svg>
					</button>
					<h1 className="text-base font-bold tracking-tight">Histórico de Salários</h1>
				</div>

				<button
					onClick={handleOpenCreate}
					className="px-3.5 py-1.5 bg-blue-600 hover:bg-blue-500 active:bg-blue-700 text-xs font-semibold rounded-lg shadow transition flex items-center gap-1.5 cursor-pointer"
				>
					<svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
						<path strokeLinecap="round" strokeLinejoin="round" d="M12 4v16m8-8H4" />
					</svg>
					Adicionar Salário
				</button>
			</header>

			{/* Main Content */}
			<main className="flex-1 overflow-auto p-6 max-w-5xl w-full mx-auto">
				{errorMsg && (
					<div className="mb-4 p-3.5 bg-red-950/60 border border-red-500/20 text-red-200 text-sm rounded-lg">
						{errorMsg}
					</div>
				)}

				{isLoading ? (
					<div className="flex items-center justify-center py-20">
						<div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500" />
					</div>
				) : wages.length === 0 ? (
					<div className="flex flex-col items-center justify-center py-20 gap-3 rounded-2xl border-2 border-dashed border-white/10 bg-white/5">
						<p className="text-gray-400 text-sm">Nenhum salário registrado no histórico.</p>
						<button
							onClick={handleOpenCreate}
							className="px-3 py-1.5 bg-white/10 hover:bg-white/15 text-xs font-semibold rounded-md transition"
						>
							Cadastrar o primeiro
						</button>
					</div>
				) : (
					<div className="overflow-hidden rounded-xl border border-white/15 bg-black/20 shadow-2xl">
						<table className="w-full text-left border-collapse">
							<thead>
								<tr className="bg-white/5 text-xs font-semibold text-gray-400 uppercase tracking-wider border-b border-white/10">
									<th className="py-3 px-4">Data Pagamento</th>
									<th className="py-3 px-4">Salário Bruto</th>
									<th className="py-3 px-4">Salário Líquido</th>
									<th className="py-3 px-4 text-right">Ações</th>
								</tr>
							</thead>
							<tbody className="divide-y divide-white/5 text-sm">
								{wages.map((wage) => (
									<tr key={wage.id} className="hover:bg-white/5 transition">
										<td className="py-3.5 px-4 font-medium">{formatDate(wage.paymentDate)}</td>
										<td className="py-3.5 px-4 text-gray-300">{formatCurrency(wage.grossValue)}</td>
										<td className="py-3.5 px-4 text-green-400 font-bold">{formatCurrency(wage.netValue)}</td>
										<td className="py-3.5 px-4 text-right flex justify-end gap-2.5">
											<button
												onClick={() => handleOpenEdit(wage.id)}
												className="p-1 rounded text-gray-400 hover:text-blue-400 hover:bg-white/5 transition cursor-pointer"
												title="Editar"
											>
												<svg xmlns="http://www.w3.org/2000/svg" className="h-4.5 w-4.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
													<path strokeLinecap="round" strokeLinejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
												</svg>
											</button>
											<button
												onClick={() => handleDelete(wage.id)}
												className="p-1 rounded text-gray-400 hover:text-red-400 hover:bg-white/5 transition cursor-pointer"
												title="Excluir"
											>
												<svg xmlns="http://www.w3.org/2000/svg" className="h-4.5 w-4.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
													<path strokeLinecap="round" strokeLinejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
												</svg>
											</button>
										</td>
									</tr>
								))}
							</tbody>
						</table>
					</div>
				)}
			</main>

			{/* Modal de Cadastro / Edição */}
			{isModalOpen && (
				<div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-50 animate-in fade-in duration-200">
					<div className="bg-[#18181b] border border-white/10 rounded-2xl w-full max-w-lg overflow-hidden shadow-2xl flex flex-col max-h-[90vh]">
						<div className="p-5 border-b border-white/10 flex items-center justify-between bg-white/5">
							<h3 className="text-base font-bold">
								{editingWageId ? "Editar Salário" : "Adicionar Salário"}
							</h3>
							<button
								onClick={() => setIsModalOpen(false)}
								className="text-gray-400 hover:text-white transition cursor-pointer"
							>
								<svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
									<path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
								</svg>
							</button>
						</div>

						<form onSubmit={handleSave} className="flex-1 overflow-y-auto p-5 flex flex-col gap-4">
							<div className="grid grid-cols-2 gap-4">
								{/* Data de Pagamento */}
								<div className="flex flex-col gap-1">
									<label className="text-xs text-gray-400 font-semibold uppercase">Data de Pagamento</label>
									<input
										type="date"
										required
										value={paymentDate}
										onChange={(e) => setPaymentDate(e.target.value)}
										className="bg-black/40 border border-white/10 rounded-lg px-3 py-2 text-sm text-white focus:outline-none focus:border-blue-500 transition"
									/>
								</div>

								{/* Salário Bruto */}
								<div className="flex flex-col gap-1">
									<label className="text-xs text-gray-400 font-semibold uppercase">Salário Bruto</label>
									<input
										type="number"
										step="0.01"
										required
										placeholder="0.00"
										value={grossValue}
										onChange={(e) => setGrossValue(e.target.value)}
										className="bg-black/40 border border-white/10 rounded-lg px-3 py-2 text-sm text-white focus:outline-none focus:border-blue-500 transition font-mono"
									/>
								</div>
							</div>

							{/* Deduções Section */}
							<div className="border-t border-white/10 pt-4 flex flex-col gap-3">
								<h4 className="text-xs text-gray-400 font-bold uppercase tracking-wider">Deduções / Descontos</h4>

								{/* Deduction Form */}
								<div className="grid grid-cols-5 gap-2 items-end">
									<div className="col-span-3 flex flex-col gap-1">
										<input
											type="text"
											placeholder="Descrição (ex: INSS, IRPF)"
											value={deductionDesc}
											onChange={(e) => setDeductionDesc(e.target.value)}
											className="bg-black/40 border border-white/10 rounded-lg px-3 py-1.5 text-xs text-white focus:outline-none focus:border-blue-500 transition"
										/>
									</div>
									<div className="col-span-1.5 flex flex-col gap-1">
										<input
											type="number"
											step="0.01"
											placeholder="Valor"
											value={deductionValue}
											onChange={(e) => setDeductionValue(e.target.value)}
											className="bg-black/40 border border-white/10 rounded-lg px-3 py-1.5 text-xs text-white focus:outline-none focus:border-blue-500 transition font-mono"
										/>
									</div>
									<button
										type="button"
										onClick={handleAddDeduction}
										className="px-2 py-1.5 bg-blue-600 hover:bg-blue-500 text-xs font-semibold rounded-lg text-white transition flex items-center justify-center cursor-pointer"
										title="Adicionar dedução"
									>
										<svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
											<path strokeLinecap="round" strokeLinejoin="round" d="M12 4v16m8-8H4" />
										</svg>
									</button>
								</div>

								{/* Deductions List */}
								{deductions.length > 0 ? (
									<div className="max-h-32 overflow-y-auto border border-white/5 bg-black/10 rounded-lg divide-y divide-white/5">
										{deductions.map((d, index) => (
											<div key={index} className="flex items-center justify-between px-3 py-2 text-xs">
												<span className="text-gray-300 font-medium">{d.description}</span>
												<div className="flex items-center gap-2">
													<span className="text-red-400 font-bold font-mono">-{formatCurrency(d.value)}</span>
													<button
														type="button"
														onClick={() => handleRemoveDeduction(index)}
														className="text-gray-500 hover:text-red-400 p-0.5 transition cursor-pointer"
													>
														<svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
															<path strokeLinecap="round" strokeLinejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7" />
														</svg>
													</button>
												</div>
											</div>
										))}
									</div>
								) : (
									<p className="text-xs text-gray-500 italic text-center py-2">Nenhuma dedução adicionada.</p>
								)}
							</div>

							{/* Summary Area */}
							<div className="mt-2 p-3 bg-black/40 border border-white/5 rounded-xl flex items-center justify-between">
								<span className="text-xs text-gray-400 uppercase font-semibold">Salário Líquido Estimado</span>
								<span className="text-base font-bold text-green-400 font-mono">
									{formatCurrency(calculateNetValue())}
								</span>
							</div>

							{/* Actions */}
							<div className="mt-4 pt-4 border-t border-white/10 flex justify-end gap-3 bg-white/0">
								<button
									type="button"
									onClick={() => setIsModalOpen(false)}
									className="px-4 py-2 border border-white/10 hover:bg-white/5 rounded-lg text-xs font-semibold transition cursor-pointer"
								>
									Cancelar
								</button>
								<button
									type="submit"
									className="px-4 py-2 bg-blue-600 hover:bg-blue-500 active:bg-blue-700 rounded-lg text-xs font-semibold text-white shadow transition cursor-pointer"
								>
									Salvar
								</button>
							</div>
						</form>
					</div>
				</div>
			)}
		</div>
	);
}
