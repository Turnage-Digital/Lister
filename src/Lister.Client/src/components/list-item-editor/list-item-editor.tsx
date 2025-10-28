import * as React from "react";

import { Save } from "@mui/icons-material";
import {
  Box,
  Button,
  CircularProgress,
  Divider,
  FormControl,
  FormControlLabel,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import { DateField } from "@mui/x-date-pickers";
import { isValid } from "date-fns";

import {
  Column,
  ColumnType,
  getStatusFromName,
  ListItemDefinition,
} from "../../models";
import FormBlock from "../form-block";
import StatusChip from "../status-chip";

interface ListItemFormProps {
  definition: ListItemDefinition;
  bag: Record<string, unknown>;
  onBagChange: (next: Record<string, unknown>) => void;
}

const ensureProperty = (column: Column): string =>
  column.property ?? column.name;

const toDateValue = (value: unknown) => {
  if (!value) {
    return null;
  }
  const date = typeof value === "string" ? new Date(value) : (value as Date);
  return Number.isNaN(date.getTime()) ? null : date;
};

const ListItemFields = ({
  definition,
  bag,
  onBagChange,
}: ListItemFormProps) => {
  const handleChange = React.useCallback(
    (property: string, value: unknown) => {
      onBagChange({
        ...bag,
        [property]: value,
      });
    },
    [bag, onBagChange],
  );

  const renderField = (column: Column) => {
    const property = ensureProperty(column);
    const value = bag[property];

    switch (column.type) {
      case ColumnType.Number:
        return (
          <TextField
            type="number"
            label={column.name}
            fullWidth
            value={value ?? ""}
            onChange={(event) => handleChange(property, event.target.value)}
            InputProps={{
              sx: {
                backgroundColor: "background.paper",
              },
            }}
          />
        );

      case ColumnType.Boolean:
        return (
          <FormControlLabel
            control={
              <Switch
                checked={Boolean(value)}
                onChange={(event) =>
                  handleChange(property, event.target.checked)
                }
              />
            }
            label={column.name}
          />
        );

      case ColumnType.Date:
        return (
          <DateField
            label={column.name}
            value={toDateValue(value)}
            onChange={(newValue) => {
              if (newValue && isValid(newValue)) {
                handleChange(property, newValue.toISOString());
              } else if (!newValue) {
                handleChange(property, undefined);
              }
            }}
            slotProps={{
              textField: {
                fullWidth: true,
                InputProps: {
                  sx: {
                    backgroundColor: "background.paper",
                  },
                },
              },
            }}
          />
        );

      case ColumnType.Text:
      default:
        return (
          <TextField
            label={column.name}
            fullWidth
            value={value ?? ""}
            onChange={(event) => handleChange(property, event.target.value)}
            InputProps={{
              sx: {
                backgroundColor: "background.paper",
              },
            }}
          />
        );
    }
  };

  if (definition.columns.length === 0) {
    return (
      <Typography variant="body2" color="text.secondary">
        Add columns to this list to capture item data.
      </Typography>
    );
  }

  return (
    <Grid container spacing={{ xs: 3.5, md: 4.5 }}>
      {definition.columns.map((column) => (
        <Grid key={ensureProperty(column)} size={{ xs: 12, md: 6 }}>
          {renderField(column)}
        </Grid>
      ))}
    </Grid>
  );
};

interface StatusSelectProps {
  definition: ListItemDefinition;
  value: string | undefined;
  onChange: (nextStatus: string) => void;
}

const ListItemStatusSelect = ({
  definition,
  value,
  onChange,
}: StatusSelectProps) => (
  <FormControl fullWidth>
    <InputLabel id="list-item-status">Status</InputLabel>
    <Select
      labelId="list-item-status"
      label="Status"
      value={value ?? ""}
      onChange={(event) => onChange(event.target.value as string)}
      displayEmpty
      sx={{
        backgroundColor: (theme) => theme.palette.background.paper,
      }}
      renderValue={(selected) => {
        if (!selected) {
          return (
            <Typography variant="body2" color="text.secondary">
              Select a status
            </Typography>
          );
        }
        return (
          <StatusChip
            status={getStatusFromName(definition.statuses, selected as string)}
          />
        );
      }}
    >
      {definition.statuses.map((status) => (
        <MenuItem key={status.name} value={status.name}>
          <StatusChip status={status} />
        </MenuItem>
      ))}
    </Select>
  </FormControl>
);

interface ListItemEditorProps {
  definition: ListItemDefinition;
  bag: Record<string, unknown>;
  onBagChange: (next: Record<string, unknown>) => void;
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => Promise<void> | void;
  isSubmitting?: boolean;
  onCancel?: () => void;
}

export const ListItemEditor = ({
  definition,
  bag,
  onBagChange,
  onSubmit,
  isSubmitting,
  onCancel,
}: ListItemEditorProps) => {
  const handleStatusChange = React.useCallback(
    (nextStatus: string) => {
      onBagChange({
        ...bag,
        status: nextStatus,
      });
    },
    [bag, onBagChange],
  );

  const handleSubmit = async (
    event: React.FormEvent<HTMLFormElement>,
  ): Promise<void> => {
    event.preventDefault();
    await onSubmit(event);
  };

  const submitStartIcon = isSubmitting ? (
    <CircularProgress size={20} color="inherit" />
  ) : (
    <Save />
  );

  return (
    <Stack
      component="form"
      onSubmit={handleSubmit}
      divider={<Divider sx={{ my: { xs: 5, md: 6 } }} />}
      spacing={{ xs: 6, md: 7 }}
    >
      <FormBlock
        title="Item details"
        subtitle="Enter values for each column and pick the current status."
        content={
          <Stack spacing={{ xs: 3.5, md: 4.5 }}>
            <ListItemFields
              definition={definition}
              bag={bag}
              onBagChange={onBagChange}
            />

            <ListItemStatusSelect
              definition={definition}
              value={bag.status as string | undefined}
              onChange={handleStatusChange}
            />
          </Stack>
        }
      />

      <Box
        sx={{
          display: "flex",
          justifyContent: { xs: "center", md: "flex-end" },
          gap: 2,
        }}
      >
        {onCancel && (
          <Button variant="text" onClick={onCancel}>
            Cancel
          </Button>
        )}
        <Button
          type="submit"
          variant="contained"
          startIcon={submitStartIcon}
          disabled={isSubmitting}
        >
          Submit
        </Button>
      </Box>
    </Stack>
  );
};

export default ListItemEditor;
