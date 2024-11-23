import * as React from "react";

import {
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Stack,
} from "@mui/material";

import { getStatusFromName, ListItem, ListItemDefinition } from "../models";
import StatusChip from "./status-chip";

interface Props {
  listItemDefinition: ListItemDefinition;
  item: ListItem;
  onItemUpdated: (key: string, value: string) => void;
}

const EditListItemStatusesContent = ({
  listItemDefinition,
  item,
  onItemUpdated,
}: Props) => (
  <Stack spacing={2}>
    <FormControl variant="outlined" fullWidth>
      <InputLabel htmlFor="status">Status</InputLabel>
      <Select
        name="status"
        id="status"
        label="Status"
        variant="outlined"
        sx={{
          background: "white",
        }}
        renderValue={(value) => (
          <StatusChip
            status={getStatusFromName(listItemDefinition.statuses, value)}
          />
        )}
        value={item.bag.status ?? ""}
        onChange={(event) => onItemUpdated("status", event.target.value)}
      >
        {listItemDefinition.statuses.map((status) => (
          <MenuItem key={status.name} value={status.name}>
            {status.name}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  </Stack>
);

export default EditListItemStatusesContent;
