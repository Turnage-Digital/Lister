import { Column } from "./column";
import { Status } from "./status";
import { StatusTransition } from "./status-transition";

export interface ListItemDefinition {
  id: string | null;
  name: string;
  columns: Column[];
  statuses: Status[];
  transitions?: StatusTransition[];
}
