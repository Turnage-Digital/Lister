import { Column } from "./column";
import { Status } from "./status";

export interface List {
  id: string | null;
  userId: string;
  name: string;
  statuses: Status[];
  columns: Column[];
  items: Item[];
}

export interface Item {
  id: string | null;
  bag: any;
}
