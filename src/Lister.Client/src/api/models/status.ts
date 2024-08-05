export interface Status {
  name: string;
  color: string;
}

export const getStatusFromName = (
  statuses: Status[],
  name: string
): Status | undefined => {
  const retval = statuses.find((status) => status.name === name);
  if (!retval) {
    return undefined;
  }
  return retval;
};
