export interface CheckResult {
  name: string;
  passed: boolean;
  message: string;
}

export interface EvaluationResult {
  passed: boolean;
  generatedEmail: string;
  checks: CheckResult[];
}

export interface BatchRegressionResult {
  totalTests: number;
  passedTests: number;
  failedTests: number;
  passRate: number;
  results: EvaluationResult[];
}

export interface PromptComparisonResult {
  v1: BatchRegressionResult;
  v2: BatchRegressionResult;
  improvement: number;
}
