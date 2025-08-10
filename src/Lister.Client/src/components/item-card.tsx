import * as React from "react";

import { Card, CardContent, Divider, Grid, Typography } from "@mui/material";

import {
  ColumnType,
  getStatusFromName,
  ListItem,
  ListItemDefinition,
} from "../models";
import StatusChip from "./status-chip";

export interface Props {
  item: ListItem;
  definition: ListItemDefinition;
}

const ItemCard = ({ item, definition }: Props) => {
  const rawStatus = item.bag.status;
  const status = getStatusFromName(definition.statuses, rawStatus);
  const statusChip = status && <StatusChip status={status} />;

  const getDisplayValue = (
    rawValue: any,
    type: ColumnType,
  ): React.ReactNode => {
    if (rawValue === null || rawValue === undefined || rawValue === "") {
      return "-";
    }

    switch (type) {
      case ColumnType.Date:
        return new Date(rawValue).toLocaleDateString();
      case ColumnType.Boolean:
        return rawValue ? "True" : "False";
      case ColumnType.Number:
      case ColumnType.Text:
      default:
        return String(rawValue);
    }
  };

  return (
    <Card>
      <CardContent>
        <Grid container spacing={1} sx={{ m: 1 }}>
          {definition.columns.map((column) => {
            const key = column.property ?? column.name;
            const rawValue = item.bag[key];
            const displayValue = getDisplayValue(rawValue, column.type);

            return (
              <React.Fragment key={key}>
                <Grid size={{ xs: 4 }}>
                  <Typography variant="subtitle2">{column.name}</Typography>
                </Grid>
                <Grid size={{ xs: 8 }}>
                  <Typography variant="body1">{displayValue}</Typography>
                </Grid>
                <Grid size={{ xs: 12 }} sx={{ my: 1 }}>
                  <Divider sx={{ mx: -3 }} />
                </Grid>
              </React.Fragment>
            );
          })}
          {statusChip && (
            <Grid size={{ xs: 12 }} sx={{ mt: 1 }}>
              {statusChip}
            </Grid>
          )}
        </Grid>
      </CardContent>
    </Card>
  );
};

export default ItemCard;
