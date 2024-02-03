import { Status } from "./models";

export const getStatusFromName = (statuses: Status[], name: string): Status => {
  const retval = statuses.find((status) => status.name === name);
  if (!retval) {
    throw new Error(`Status with name ${name} not found`);
  }
  return retval;
};
