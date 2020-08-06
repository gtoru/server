import { Question } from "../task/models";
import { Email } from "../auth/models";

export type TestSessionId = string;

export type Guess = string;

export type TaskNumber = number;

export type Result = number;

export type CorrectAnswer = string;

export class TaskAnswer {
    public taskNumber: TaskNumber;
    public answer: Guess;
}

export class QuizTask {
    public question: Question;
    public answers: CorrectAnswer[];
}

export class TestSessionInfo {
    public testSessionId: TestSessionId;
    public tasks: QuizTask[];
}

export class TestSessionResult {
    testSessionId: TestSessionId;
    result: Result;
}

export class UserStats {
    userMail: Email;
    result: Result;
}
