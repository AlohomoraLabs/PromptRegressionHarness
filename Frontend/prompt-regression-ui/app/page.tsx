"use client";

import { FormEvent, useState } from "react";
import type { BatchRegressionResult, EvaluationResult, PromptComparisonResult } from "@/src/types/regression";

const defaultPromptV1 = "Write a short email to the homeowner about selling their property.";
const defaultPromptV2 = `Write a short, friendly and professional email to the homeowner about potentially selling their property.

Use ONLY the homeowner information provided below.

You MUST:
- Use the homeowner's name.
- Use the exact property address.
- Use the ownership duration only if it is provided.
- Use the estimated equity only if it is provided.
- Include one clear call to action.
- Include an unsubscribe option.
- Keep the email under 500 characters.

You MUST NOT:
- Invent property details.
- Invent buyers, offers, valuations, market research, agents, companies, or previous conversations.
- Assume facts that are not explicitly provided.
- Use aggressive, urgent, or exaggerated sales language.

Return only the email.`;

function formatRate(rate: number) { return `${rate.toFixed(0)}%`; }
function formatImprovement(improvement: number) { return `${improvement > 0 ? "+" : ""}${improvement.toFixed(0)}%`; }

function SummaryCard({ label, result, value }: { label: string; result?: BatchRegressionResult; value?: string }) {
  return <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm"><p className="text-sm font-medium text-slate-500">{label}</p><p className="mt-2 text-3xl font-semibold tracking-tight text-slate-900">{value ?? formatRate(result?.passRate ?? 0)}</p>{result && <p className="mt-2 text-sm text-slate-500">{result.passedTests} passed � {result.failedTests} failed of {result.totalTests}</p>}</div>;
}
function StatusBadge({ passed }: { passed: boolean }) { return <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-bold tracking-wide ${passed ? "bg-emerald-100 text-emerald-800" : "bg-rose-100 text-rose-800"}`}>{passed ? "PASS" : "FAIL"}</span>; }
function TestColumn({ label, result }: { label: string; result?: EvaluationResult }) {
  if (!result) return <p className="text-sm text-slate-500">No result returned.</p>;
  return <div className="rounded-lg border border-slate-200 bg-slate-50 p-4"><div className="flex items-center justify-between gap-3"><h4 className="font-semibold text-slate-800">{label}</h4><StatusBadge passed={result.passed} /></div><p className="mt-3 whitespace-pre-wrap rounded-md border border-slate-200 bg-white p-3 text-sm leading-6 text-slate-700">{result.generatedEmail || "No email was generated."}</p><ul className="mt-3 space-y-2">{result.checks.map((check) => <li key={`${label}-${check.name}`} className="flex gap-2 text-sm"><span className={check.passed ? "text-emerald-600" : "text-rose-600"} aria-hidden="true">{check.passed ? "?" : "?"}</span><span className="text-slate-700"><span className="font-medium">{check.name}:</span> {check.message}</span></li>)}</ul></div>;
}
function Results({ comparison }: { comparison: PromptComparisonResult }) {
  const count = Math.max(comparison.v1.results.length, comparison.v2.results.length);
  return <section className="mt-10" aria-labelledby="results-heading"><div className="flex flex-wrap items-end justify-between gap-3"><div><p className="text-sm font-semibold uppercase tracking-widest text-indigo-600">Evaluation</p><h2 id="results-heading" className="mt-1 text-2xl font-semibold text-slate-900">Regression results</h2></div><p className="text-sm text-slate-500">{count} test result{count === 1 ? "" : "s"}</p></div><div className="mt-5 space-y-4">{count === 0 ? <div className="rounded-xl border border-slate-200 bg-white p-6 text-sm text-slate-600">The API returned no individual test results.</div> : Array.from({ length: count }, (_, index) => <details key={index} className="group rounded-xl border border-slate-200 bg-white shadow-sm"><summary className="flex cursor-pointer list-none items-center justify-between gap-4 p-5 font-semibold text-slate-800 marker:content-none"><span>Test {index + 1}</span><span className="text-sm font-normal text-slate-500 group-open:hidden">View details</span><span className="hidden text-sm font-normal text-slate-500 group-open:inline">Hide details</span></summary><div className="grid gap-4 border-t border-slate-100 p-5 md:grid-cols-2"><TestColumn label="Prompt V1" result={comparison.v1.results[index]} /><TestColumn label="Prompt V2" result={comparison.v2.results[index]} /></div></details>)}</div></section>;
}

export default function Home() {
  const [promptV1, setPromptV1] = useState(defaultPromptV1); const [promptV2, setPromptV2] = useState(defaultPromptV2); const [comparison, setComparison] = useState<PromptComparisonResult | null>(null); const [error, setError] = useState<string | null>(null); const [isRunning, setIsRunning] = useState(false);
  async function runComparison(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError(null); setComparison(null); const apiUrl = process.env.NEXT_PUBLIC_API_URL;
    if (!apiUrl) { setError("The regression API URL is not configured. Set NEXT_PUBLIC_API_URL in .env.local."); return; }
    setIsRunning(true);
    try { const response = await fetch(`${apiUrl.replace(/\/$/, "")}/api/Regression/compare`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ promptV1, promptV2 }) }); if (!response.ok) throw new Error(); setComparison((await response.json()) as PromptComparisonResult); } catch { setError("Could not connect to the regression API."); } finally { setIsRunning(false); }
  }
  const prompts = [{ label: "Prompt V1", value: promptV1, setValue: setPromptV1 }, { label: "Prompt V2", value: promptV2, setValue: setPromptV2 }];
  return <main className="mx-auto w-full max-w-7xl px-5 py-10 sm:px-8 lg:py-14"><header className="max-w-3xl"><p className="text-sm font-semibold uppercase tracking-widest text-indigo-600">Developer evaluation dashboard</p><h1 className="mt-3 text-3xl font-semibold tracking-tight text-slate-950 sm:text-4xl">Prompt Regression Harness</h1><p className="mt-3 text-lg text-slate-600">Compare two prompt versions against the same test data.</p></header><form onSubmit={runComparison} className="mt-10"><div className="grid gap-6 lg:grid-cols-2">{prompts.map((prompt) => <label key={prompt.label} className="block rounded-xl border border-slate-200 bg-white p-5 shadow-sm"><span className="text-base font-semibold text-slate-900">{prompt.label}</span><textarea value={prompt.value} onChange={(event) => prompt.setValue(event.target.value)} className="mt-3 min-h-80 w-full resize-y rounded-lg border border-slate-300 bg-slate-50 p-4 font-mono text-sm leading-6 text-slate-800 outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100" aria-label={prompt.label} /></label>)}</div><div className="mt-7 flex justify-center"><button type="submit" disabled={isRunning} className="rounded-lg bg-indigo-600 px-7 py-3 font-semibold text-white shadow-sm transition hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:bg-indigo-300">{isRunning ? "Running..." : "Run Comparison"}</button></div></form>{error && <div role="alert" className="mt-7 rounded-lg border border-rose-200 bg-rose-50 p-4 text-sm text-rose-800">{error}</div>}{comparison && <><section className="mt-12 grid gap-4 sm:grid-cols-3" aria-label="Comparison summary"><SummaryCard label="V1 Pass Rate" result={comparison.v1} /><SummaryCard label="V2 Pass Rate" result={comparison.v2} /><SummaryCard label="Improvement" value={formatImprovement(comparison.improvement)} /></section><Results comparison={comparison} /></>}</main>;
}
