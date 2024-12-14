import * as React from "react";

import { Stack, TextField } from "@mui/material";
import { DateField } from "@mui/x-date-pickers";
import { isValid } from "date-fns";

import { ColumnType, ListItem, ListItemDefinition } from "../models";

interface Props {
  listItemDefinition: ListItemDefinition;
  item: ListItem;
  onItemUpdated: (key: string, value: any) => void;
}

const EditListItemColumnContent = ({
  listItemDefinition,
  item,
  onItemUpdated,
}: Props) => {
  const getDate = (date: number | null) => {
    return date ? new Date(date) : null;
  };

  const handleDateChange = (property: string, newValue: Date | null) => {
    if (newValue && isValid(newValue)) {
      onItemUpdated(property, newValue.toISOString());
    }
  };

  return (
    <Stack spacing={2}>
      {listItemDefinition.columns.map((column) => {
        switch (column.type) {
          case ColumnType.Text:
            return (
              <TextField
                key={column.name}
                label={column.name}
                sx={{
                  background: "white",
                }}
                value={item.bag[column.property!] ?? ""}
                onChange={(e) =>
                  onItemUpdated(column.property!, e.target.value)
                }
              />
            );

          case ColumnType.Number:
            return (
              <TextField
                key={column.name}
                label={column.name}
                type="number"
                sx={{
                  background: "white",
                }}
                value={item.bag[column.property!] ?? ""}
                onChange={(e) =>
                  onItemUpdated(column.property!, e.target.value)
                }
              />
            );

          case ColumnType.Date:
            return (
              <DateField
                key={column.name}
                label={column.name}
                sx={{
                  background: "white",
                }}
                value={getDate(item.bag[column.property!])}
                onChange={(newValue) =>
                  handleDateChange(column.property!, newValue)
                }
              />
            );

          default:
            return null;
        }
      })}
    </Stack>
  );
};
export default EditListItemColumnContent;
