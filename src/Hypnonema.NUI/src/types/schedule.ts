import Screen from "./screen";

export enum Recurrence {
  Monthly = 1,
  Weekly,
  Daily,
  Hourly,
}

export enum DayOfWeek {
  Monday,
  Tuesday,
  Wednesday,
  Thursday,
  Friday,
  Saturday,
  Sunday,
}

export default interface Schedule {
  id?: number;
  startDateTime: string;
  endDate?: string;
  rule: Recurrence;
  interval: number;
  dayOfWeek?: DayOfWeek;
  url: string;
  screen: Screen;
}
