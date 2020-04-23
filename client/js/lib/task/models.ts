export type Question = string;

export type Answer = string;

export type TaskId = string;

export type TaskWeight = number;

export class Task {
    public weight: TaskWeight;
    public taskId: TaskId;
    public question: Question;
    public answer: Answer;
    public variants: Answer[];
}
