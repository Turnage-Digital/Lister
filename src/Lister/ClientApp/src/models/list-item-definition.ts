import { Column } from "./column";
import { Status } from "./status";

export interface ListItemDefinition {
  id: string | null;
  columns: Column[];
  statuses: Status[];
}
