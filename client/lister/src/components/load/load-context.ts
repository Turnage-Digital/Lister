import { createContext } from "react";

export interface LoadValue {
  loading: boolean;
  setLoading: (loading: boolean) => void;
}

const defaultValue: LoadValue = {
  loading: false,
  setLoading: () => {},
};

const LoadContext = createContext<LoadValue>(defaultValue);

export default LoadContext;
