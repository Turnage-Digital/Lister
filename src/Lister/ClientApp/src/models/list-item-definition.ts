import { Column } from "./column";
import { Status } from "./status";

export interface ListItemDefinition {
  id: string | null;
  name: string;
  columns: Column[];
  statuses: Status[];
}
