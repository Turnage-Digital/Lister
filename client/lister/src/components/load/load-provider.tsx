import React, { PropsWithChildren, useMemo, useState } from "react";

import LoadContext from "./load-context";

const LoadProvider = ({ children }: PropsWithChildren) => {
  const [loading, setLoading] = useState(false);

  const value = useMemo(
    () => ({
      loading,
      setLoading,
    }),
    [loading]
  );

  return <LoadContext.Provider value={value}>{children}</LoadContext.Provider>;
};

export default LoadProvider;
