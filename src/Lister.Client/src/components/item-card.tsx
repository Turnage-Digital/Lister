import * as React from "react";

import {
  CalendarToday,
  CheckCircle,
  Numbers,
  TextFields,
} from "@mui/icons-material";
import {
  Card,
  CardContent,
  Divider,
  Grid,
  Typography,
  Box,
  alpha,
  useTheme,
} from "@mui/material";

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
  const theme = useTheme();
  const rawStatus = item.bag.status;
  const status = getStatusFromName(definition.statuses, rawStatus);
  const statusChip = status && <StatusChip status={status} />;

  const getFieldIcon = (type: ColumnType) => {
    switch (type) {
      case ColumnType.Date:
        return <CalendarToday sx={{ fontSize: 16, color: "text.secondary" }} />;
      case ColumnType.Boolean:
        return <CheckCircle sx={{ fontSize: 16, color: "text.secondary" }} />;
      case ColumnType.Number:
        return <Numbers sx={{ fontSize: 16, color: "text.secondary" }} />;
      case ColumnType.Text:
      default:
        return <TextFields sx={{ fontSize: 16, color: "text.secondary" }} />;
    }
  };

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
    <Card
      variant="outlined"
      sx={{
        transition: "all 0.2s ease-in-out",
        "&:hover": {
          transform: "translateY(-2px)",
          boxShadow: theme.shadows[4],
          borderColor: alpha(theme.palette.primary.main, 0.5),
        },
      }}
    >
      <CardContent sx={{ p: 3 }}>
        {/* Status Section */}
        {statusChip && (
          <Box sx={{ mb: 3, display: "flex", justifyContent: "flex-end" }}>
            {statusChip}
          </Box>
        )}

        {/* Fields Grid */}
        <Grid container spacing={2}>
          {definition.columns.map((column, index) => {
            const key = column.property ?? column.name;
            const rawValue = item.bag[key];
            const displayValue = getDisplayValue(rawValue, column.type);
            const isLastItem = index === definition.columns.length - 1;

            return (
              <React.Fragment key={key}>
                <Grid size={{ xs: 4 }}>
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                      mb: 0.5,
                    }}
                  >
                    {getFieldIcon(column.type)}
                    <Typography
                      variant="caption"
                      color="text.secondary"
                      fontWeight="medium"
                      sx={{ textTransform: "uppercase", letterSpacing: 0.5 }}
                    >
                      {column.name}
                    </Typography>
                  </Box>
                </Grid>
                <Grid size={{ xs: 8 }}>
                  <Typography
                    variant="body1"
                    color="text.primary"
                    fontWeight={
                      column.type === ColumnType.Number ? "medium" : "normal"
                    }
                    sx={{
                      fontSize: "1rem",
                      lineHeight: 1.5,
                    }}
                  >
                    {displayValue}
                  </Typography>
                </Grid>
                {!isLastItem && (
                  <Grid size={{ xs: 12 }} sx={{ my: 1.5 }}>
                    <Divider
                      sx={{
                        mx: -3,
                        borderColor: alpha(theme.palette.divider, 0.6),
                      }}
                    />
                  </Grid>
                )}
              </React.Fragment>
            );
          })}
        </Grid>
      </CardContent>
    </Card>
  );
};

export default ItemCard;
