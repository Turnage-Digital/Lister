import * as React from "react";

import { Box, Card, CardContent, Grid, Typography } from "@mui/material";

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

  return (
    <Card>
      <CardContent>
        <Box component="dl">
          {definition.columns.map((column) => {
            const key = column.property ?? column.name;
            const rawValue = item.bag[key];
            let displayValue: React.ReactNode = "-";

            switch (column.type) {
              case ColumnType.Date:
                displayValue = rawValue
                  ? new Date(rawValue).toLocaleDateString()
                  : "-";
                break;
              case ColumnType.Boolean:
                displayValue = rawValue ? "Yes" : "No";
                break;
              case ColumnType.Number:
                displayValue = rawValue ?? "-";
                break;
              case ColumnType.Text:
                displayValue = rawValue ?? "-";
                break;
            }

            return (
              <Grid container key={key} spacing={1} sx={{ mb: 1 }}>
                <Grid size={{ xs: 4 }}>
                  <Typography component="dt" variant="subtitle2">
                    {column.name}
                  </Typography>
                </Grid>
                <Grid size={{ xs: 8 }}>
                  <Typography component="dd" variant="body1">
                    {displayValue}
                  </Typography>
                </Grid>
              </Grid>
            );
          })}
        </Box>

        {statusChip && (
          <Grid container spacing={1} sx={{ mt: 4 }}>
            <Grid size={{ xs: 12 }}>{statusChip}</Grid>
          </Grid>
        )}
      </CardContent>
    </Card>
  );
};

export default ItemCard;
