using System;
using System.Collections.Generic;
using server.core.Domain.Error;
using server.core.Domain.Misc;

namespace server.core.Domain.Tasks
{
    public class TestSession
    {
        private static readonly TimeSpan TimeToComplete = TimeSpan.FromMinutes(30);

        private readonly ITimeProvider _timeProvider;

        public TestSession()
        {
        }

        public TestSession(
            Guid sessionId,
            DateTime started,
            bool isFinished,
            int result,
            int possibleResult,
            Quiz quiz,
            List<string> answers,
            ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
            SessionId = sessionId;
            Started = started;
            IsFinished = isFinished;
            Result = result;
            PossibleResult = possibleResult;
            Quiz = quiz;
            Answers = answers;
        }

        public Guid SessionId { get; set; }
        public DateTime Started { get; set; }
        public bool IsFinished { get; set; }
        public int Result { get; set; }
        public int PossibleResult { get; set; }
        public Quiz Quiz { get; set; }
        public List<string> Answers { get; set; }

        public static TestSession StartNew(Quiz quiz, ITimeProvider timeProvider = null)
        {
            var id = Guid.NewGuid();

            timeProvider ??= new UtcTimeProvider();

            quiz.Lock();

            var possibleResult = quiz.Tasks.Count;

            var answers = new List<string>(possibleResult);

            for (var i = 0; i < possibleResult; ++i)
                answers.Add("#");

            var started = timeProvider.GetCurrent();

            return new TestSession(
                id,
                started,
                false,
                0,
                possibleResult,
                quiz,
                answers,
                timeProvider);
        }

        public void Answer(int taskNumber, string guess)
        {
            if (taskNumber < 0 || taskNumber >= Answers.Count)
                throw new TaskNumberOutOfRangeException();

            if (IsFinished || Started + TimeToComplete < _timeProvider.GetCurrent())
            {
                IsFinished = true;
                throw new TestSessionAlreadyFinishedException();
            }

            Answers[taskNumber] = guess;
        }

        public void Finish()
        {
            if (IsFinished)
                return;

            IsFinished = true;

            for (var i = 0; i < Answers.Count; ++i)
                if (Quiz.Tasks[i].CheckAnswer(Answers[i]))
                    Result += 1;
        }
    }
}
