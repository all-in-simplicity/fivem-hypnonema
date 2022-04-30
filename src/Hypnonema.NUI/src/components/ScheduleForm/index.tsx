import { FC } from "react";

import {
  Button,
  FormControl,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import Schedule, { DayOfWeek, Recurrence } from "../../types/schedule";
import enLocale from "date-fns/locale/en-GB";
import {
  DatePicker,
  DateTimePicker,
  LocalizationProvider,
} from "@mui/x-date-pickers";

import Screen from "../../types/screen";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { useFormik } from "formik";
import { parseISO } from "date-fns";

const { DateTime } = require("luxon");

interface ScheduleFormProps {
  schedule?: Schedule | null;
  screens: Screen[] | undefined;
  onSubmitHandler: Function;
}

interface ScheduleFormErrorType {
  url?: string | undefined;
  startDateTime?: string | undefined;
  endDate?: string | undefined;
  rule?: string | undefined;
  interval?: string | undefined;
  dayOfWeek?: string | undefined;
  screenName?: string | undefined;
}

const isValidDate = (d: string | number | Date | null) => {
  return d instanceof Date && !isNaN(d as unknown as number);
};

export const ScheduleForm: FC<ScheduleFormProps> = (props) => {
  const formik = useFormik({
    initialValues: {
      endDate:
        props.schedule && props.schedule.endDate
          ? parseISO(props.schedule.endDate)
          : null,
      rule: props.schedule ? props.schedule.rule : Recurrence.Daily,
      interval: props.schedule ? props.schedule.interval : 1,
      dayOfWeek:
        props.schedule && props.schedule.dayOfWeek
          ? props.schedule.dayOfWeek
          : DayOfWeek.Monday,
      url: props.schedule ? props.schedule.url : "",
      screenName: props.schedule ? props.schedule.screen.name : "",
      startDateTime: props.schedule
        ? parseISO(props.schedule.startDateTime)
        : new Date(),
    },
    validate: (values) => {
      const errors: ScheduleFormErrorType = {};

      if (!values.url) {
        errors.url = "Url is required";
      }

      if (!values.screenName) {
        errors.screenName = "Screen is required";
      }

      if (!values.rule) {
        errors.rule = "Frequency is required";
      }

      if (!values.startDateTime) {
        errors.startDateTime = "Start Time is required";
      }

      if (!isValidDate(values.startDateTime)) {
        errors.startDateTime = "Start Time is invalid";
      }

      if (isNaN(parseInt(values.interval.toString()))) {
        formik.setFieldValue("interval", 1);
        errors.interval = "Interval is required";
      }

      try {
        new URL(values.url);
      } catch (e) {
        errors.url = "URL is invalid";
      }

      return errors;
    },
    onSubmit: (values) => {
      props.onSubmitHandler(values);
    },
  });

  return (
    <div>
      <form onSubmit={formik.handleSubmit}>
        <Grid
          container
          spacing={{ xs: 2, md: 2 }}
          columns={{ xs: 2, sm: 8, md: 12 }}
        >
          <Grid item xs={6} md={11}>
            <TextField
              value={formik.values.url}
              fullWidth
              error={formik.touched.url && Boolean(formik.errors.url)}
              helperText={formik.touched.url && formik.errors.url}
              onChange={formik.handleChange}
              label="URL"
              name="url"
              sx={{ maxWidth: 518, width: 518 }}
            />
          </Grid>
          <Grid item>
            <LocalizationProvider
              dateAdapter={AdapterDateFns}
              locale={enLocale}
            >
              <DateTimePicker
                label="Start Time"
                disablePast
                onChange={(newValue) => {
                  formik.setFieldValue("startDateTime", newValue);
                }}
                value={formik.values.startDateTime}
                renderInput={(params) => (
                  <TextField
                    error={
                      formik.touched.startDateTime &&
                      Boolean(formik.errors.startDateTime)
                    }
                    helperText={
                      formik.touched.startDateTime &&
                      formik.errors.startDateTime
                    }
                    sx={{ minWidth: 250 }}
                    {...params}
                  />
                )}
              />
            </LocalizationProvider>
          </Grid>

          <Grid item>
            <FormControl sx={{ minWidth: 250, maxWidth: 250 }}>
              <InputLabel>Screen</InputLabel>
              <Select
                value={formik.values.screenName}
                label="Screen"
                error={
                  formik.touched.screenName && Boolean(formik.errors.screenName)
                }
                onChange={(event: SelectChangeEvent) => {
                  formik.setFieldValue("screenName", event.target.value);
                }}
              >
                {props.screens?.length !== 0 &&
                  props.screens?.map((screen, index) => (
                    <MenuItem key={index} value={screen.name}>
                      {screen.name}
                    </MenuItem>
                  ))}
              </Select>
              {formik.touched.screenName && formik.errors.screenName && (
                <FormHelperText className="Mui-error">
                  {formik.errors.screenName}
                </FormHelperText>
              )}
            </FormControl>
          </Grid>
          <Grid item>
            <FormControl sx={{ minWidth: 250 }}>
              <InputLabel>Frequency</InputLabel>
              <Select
                value={formik.values.rule.toString()}
                label="Frequency"
                onChange={(event: SelectChangeEvent) => {
                  formik.setFieldValue(
                    "rule",
                    parseInt(event.target.value) as Recurrence
                  );
                }}
              >
                <MenuItem value="4">Hourly</MenuItem>
                <MenuItem value="3">Daily</MenuItem>
                <MenuItem value="2">Weekly</MenuItem>
                <MenuItem value="1">Monthly</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item>
            <FormControl sx={{ minWidth: 250 }}>
              <InputLabel>Day of Week</InputLabel>
              <Select
                value={formik.values.dayOfWeek?.toString()}
                label="Day of Week"
                disabled={
                  formik.values.rule === Recurrence.Daily ||
                  formik.values.rule === Recurrence.Hourly
                }
                onChange={(event: SelectChangeEvent) => {
                  formik.setFieldValue(
                    "dayOfWeek",
                    parseInt(event.target.value) as DayOfWeek
                  );
                }}
              >
                <MenuItem value="0">Sunday</MenuItem>
                <MenuItem value="1">Monday</MenuItem>
                <MenuItem value="2">Tuesday</MenuItem>
                <MenuItem value="3">Wednesday</MenuItem>
                <MenuItem value="4">Thursday</MenuItem>
                <MenuItem value="5">Friday</MenuItem>
                <MenuItem value="6">Saturday</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item>
            <LocalizationProvider
              dateAdapter={AdapterDateFns}
              locale={enLocale}
            >
              <DatePicker
                label="Until"
                value={formik.values.endDate}
                disablePast
                minDate={formik.values.startDateTime}
                onChange={(newValue: typeof DateTime) => {
                  formik.setFieldValue("endDate", newValue);
                }}
                renderInput={(params) => (
                  <TextField
                    sx={{ minWidth: 250 }}
                    helperText="Leave empty for no end date."
                    {...params}
                  />
                )}
              />
            </LocalizationProvider>
          </Grid>
          <Grid item>
            <TextField
              type="number"
              label="Interval"
              helperText={`Eg. 2 would mean every 2nd ${
                formik.values.rule === Recurrence.Daily
                  ? "Day"
                  : formik.values.rule === Recurrence.Monthly
                  ? "Month"
                  : formik.values.rule === Recurrence.Hourly
                  ? "Hour"
                  : "Week"
              }`}
              value={formik.values.interval}
              onChange={(event) => {
                let value = parseInt(event.target.value);

                if (value <= 0) value = 1;

                formik.setFieldValue("interval", value);
              }}
              inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
              sx={{ minWidth: 250 }}
            />
          </Grid>
          <Grid item sx={{ marginTop: "auto", marginLeft: "auto" }}>
            <Button
              variant="contained"
              type="submit"
              sx={{ marginBottom: "24px", marginRight: "20px" }}
            >
              Submit
            </Button>
          </Grid>
        </Grid>
      </form>
    </div>
  );
};
