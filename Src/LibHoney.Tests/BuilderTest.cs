﻿using System;
using System.Collections.Generic;
using Xunit;

namespace Honeycomb.Tests
{
    public class BuilderTest : IClassFixture<LibHoneyFixture>, IDisposable
    {
        LibHoneyFixture fixture;

        public BuilderTest (LibHoneyFixture fixture)
        {
            this.fixture = fixture;
        }

        public void Dispose ()
        {
            fixture.LibHoney.Reset ();
        }

        LibHoney GetLibHoney ()
        {
            return fixture.LibHoney;
        }

        [Fact]
        public void CtorNull ()
        {
            bool excThrown = false;
            try { new Builder (null); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);
        }

        [Fact]
        public void CtorNull2 ()
        {
            var honey = GetLibHoney ();

            bool excThrown = false;
            try { new Builder (honey, null); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);

            excThrown = false;
            try { new Builder (null, new Dictionary<string, object> ()); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);
        }

        [Fact]
        public void CtorNull3 ()
        {
            var honey = GetLibHoney ();

            bool excThrown = false;
            try { new Builder (honey, new Dictionary<string, object> (), null); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);

            excThrown = false;
            try { new Builder (honey, null, new Dictionary<string, Func<object>> ()); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);

            excThrown = false;
            try { new Builder (null, new Dictionary<string, object> (), new Dictionary<string, Func<object>> ()); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);
        }

        [Fact]
        public void Ctor ()
        {
            var honey = GetLibHoney ();
            var b = new Builder (honey);
            Assert.Equal (honey.WriteKey, b.WriteKey);
            Assert.Equal (honey.DataSet, b.DataSet);
            Assert.Equal (honey.SampleRate, b.SampleRate);
        }

        [Fact]
        public void DataSet ()
        {
            var b = new Builder (GetLibHoney ());

            b.DataSet = null;
            Assert.Null (b.DataSet);

            b.DataSet = "unknown";
            Assert.Equal ("unknown", b.DataSet);
        }

        [Fact]
        public void SampleRate ()
        {
            var b = new Builder (GetLibHoney ());

            b.SampleRate = -1;
            Assert.Equal (-1, b.SampleRate);

            b.SampleRate = 100;
            Assert.Equal (100, b.SampleRate);
        }

        [Fact]
        public void WriteKey ()
        {
            var b = new Builder (GetLibHoney ());

            b.WriteKey = null;
            Assert.Null (b.WriteKey);

            b.WriteKey = "aaa-bbb-ccc";
            Assert.Equal ("aaa-bbb-ccc", b.WriteKey);
        }

        [Fact]
        public void AddNull ()
        {
            bool excThrown = false;
            var b = new Builder (GetLibHoney ());
            try { b.Add (null); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);
        }

        [Fact]
        public void AddFieldNull ()
        {
            bool excThrown = false;
            var b = new Builder (GetLibHoney ());
            try { b.AddField (null, "abc"); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);

            excThrown = false;
            try { b.AddField ("abc", null); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (false, excThrown);
        }

        [Fact]
        public void AddDynamicFieldNull ()
        {
            bool excThrown = false;
            var b = new Builder (GetLibHoney ());
            try { b.AddDynamicField (null, () => "abc"); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);

            excThrown = false;
            try { b.AddDynamicField ("abc", null); } catch (ArgumentNullException) { excThrown = true; }
            Assert.Equal (true, excThrown);
        }

        [Fact]
        public void Clone ()
        {
            var honey = GetLibHoney ();

            var b = new Builder (honey);
            b.WriteKey = "aaa-bbb-ccc";
            b.DataSet = "unknown";
            b.ApiHost = "https://unknown";
            b.SampleRate = 5;

            var clone = b.Clone ();
            Assert.NotSame (b, clone);
            Assert.Equal ("aaa-bbb-ccc", clone.WriteKey);
            Assert.Equal ("unknown", clone.DataSet);
            Assert.Equal ("https://unknown", clone.ApiHost);
            Assert.Equal (5, clone.SampleRate);
        }

        [Fact]
        public void NewEvent ()
        {
            var honey = GetLibHoney ();

            var b = new Builder (honey);
            var ev = b.NewEvent ();
            Assert.Equal (true, ev != null);
            Assert.Equal (honey.WriteKey, ev.WriteKey);
            Assert.Equal (honey.DataSet, ev.DataSet);
            Assert.Equal (honey.ApiHost, ev.ApiHost);
            Assert.Equal (honey.SampleRate, ev.SampleRate);
        }

        [Fact]
        public void NewEventOverrideProperties ()
        {
            var honey = GetLibHoney ();

            var b = new Builder (honey);
            b.WriteKey = "aaa-bbb-ccc";
            b.DataSet = "unknown";
            b.ApiHost = "https://unknown";
            b.SampleRate = 5;

            var ev = b.NewEvent ();
            Assert.NotNull (ev);
            Assert.Equal ("aaa-bbb-ccc", ev.WriteKey);
            Assert.Equal ("unknown", ev.DataSet);
            Assert.Equal ("https://unknown", ev.ApiHost);
            Assert.Equal (5, ev.SampleRate);
        }

        [Fact]
        public void NewEventJSON ()
        {
            var b = new Builder (GetLibHoney ());
            b.AddField ("v1", 13);
            b.AddField ("v2", new [] { 1, 7, 13 });
            b.AddDynamicField ("d1", () => 14);
            b.AddDynamicField ("d2", () => new [] { 2, 8, 14 });

            var ev = b.NewEvent ();
            Assert.Equal (true, ev != null);
            Assert.Equal ("{\"v1\":13,\"v2\":\"[1,7,13]\",\"d1\":14,\"d2\":\"[2,8,14]\"}", ev.ToJSON ());
        }

        [Fact]
        public void SendNowNull ()
        {
            var b = new Builder (GetLibHoney ());
            bool excThrown = false;
            try { b.SendNow (null); } catch (ArgumentNullException) { excThrown = true; }
            Assert.True (excThrown);
        }

        [Fact]
        public void SendNowDisposed ()
        {
            var honey = GetLibHoney ();
            var b = new Builder (honey);
            b.AddField ("key1", "value1"); // Add data to have a valid event.
            honey.Close ();

            bool excThrown = false;
            try { b.SendNow (); } catch (SendException) { excThrown = true; }
            Assert.True (excThrown);
        }

        [Fact]
        public void SendNowEmpty ()
        {
            var b = new Builder (GetLibHoney ());
            bool excThrown = false;
            try { b.SendNow (new Dictionary<string, object> ()); } catch (SendException) { excThrown = true; }
            Assert.True (excThrown);
        }
    }
}
