import { ListItem } from "./list-item";

export interface PagedList {
  id: string;
  count: number;
  items: ListItem[];
}
