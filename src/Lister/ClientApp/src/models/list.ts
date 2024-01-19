import { Column } from "./column";
import { Status } from "./status";
import { Item } from "./item";

export interface List {
  id: string | null;
  name: string;
  columns: Column[];
  statuses: Status[];
  items: Item[];
  count: number;
  createdBy: string;
  createdOn: Date;
}
