import React, { PropsWithChildren, useMemo, useState } from "react";

import LoadContext from "./load-context";

const LoadProvider = ({ children }: PropsWithChildren) => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>();

  const value = useMemo(
    () => ({
      loading,
      error,
      setLoading,
    }),
    [loading, error, setLoading]
  );

  return <LoadContext.Provider value={value}>{children}</LoadContext.Provider>;
};

export default LoadProvider;
