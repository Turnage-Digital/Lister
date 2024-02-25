import { Stack, TextField } from "@mui/material";
import React from "react";

import { Item, ListItemDefinition } from "../../models";

interface Props {
  listItemDefinition: ListItemDefinition;
  item: Item;
  onItemUpdated: (key: string, value: string) => void;
}

const ColumnContent = ({ listItemDefinition, item, onItemUpdated }: Props) => (
  <Stack spacing={2}>
    {listItemDefinition.columns.map((column) => {
      return (
        <TextField
          key={column.name}
          label={column.name}
          value={item.bag[column.property!] ?? ""}
          onChange={(e) => onItemUpdated(column.property!, e.target.value)}
        />
      );
    })}
  </Stack>
);
export default ColumnContent;
