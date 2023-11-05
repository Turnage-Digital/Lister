import { Column } from "./column";
import { Status } from "./status";

export interface List {
  id: string | null;
  userId: string;
  name: string;
  statuses: Status[];
  columns: Column[];
}
