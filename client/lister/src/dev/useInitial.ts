import { useState } from "react";
import { InitialHookStatus } from "@react-buddy/ide-toolbox";

export const useInitial: () => InitialHookStatus = () => {
  const [status, setStatus] = useState<InitialHookStatus>({
    loading: false,
    error: false,
  });

  return status;
};
