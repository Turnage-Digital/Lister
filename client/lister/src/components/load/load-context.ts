import { createContext } from "react";

export interface LoadValue {
  loading: boolean;
  error?: string;
  setLoading: (loading: boolean) => void;
  setError: (error?: string) => void;
}

const defaultValue: LoadValue = {
  loading: false,
  setLoading: () => {},
  setError: () => {},
};

const LoadContext = createContext<LoadValue>(defaultValue);

export default LoadContext;
