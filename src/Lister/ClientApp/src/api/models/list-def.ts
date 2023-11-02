import { ColumnDef } from "./column-def";
import { StatusDef } from "./status-def";

export interface ListDef {
  id: string | null;
  userId: string;
  name: string;
  statusDefs: StatusDef[];
  propertyDefs: ColumnDef[];
}
