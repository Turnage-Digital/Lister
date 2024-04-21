import { Status } from "./models";

export const getStatusFromName = (statuses: Status[], name: string): Status => {
  const retval = statuses.find((status) => status.name === name);
  if (!retval) {
    return {
      name: "Unknown",
      color: "#000",
    };
  }
  return retval;
};
